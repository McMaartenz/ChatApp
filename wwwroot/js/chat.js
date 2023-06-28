"use strict";

const KEYCODES = {
	ENTER: 13
};

const ChatSystem = (() => {
	let isSideBarOpen = false;
	const IsSideBarOpen = () => {
		return isSidebarOpen;
	};

	const ToggleSideBar = () => {
		isSideBarOpen = !isSideBarOpen;
		$("#chat-sidebar").css('left', `${isSideBarOpen * -101}%`);
	};

	const SidebarGone = () => {
		$("#chat-sidebar").css('left', `-101%`);
	}

	const Send = async (input) => {
		if (input == '') {
			return false;
		}

		connection.invoke('SendMessage', await ChatSystem.GetUserId(), currentChannelId, input).catch(function (err) {
			return console.error(err);
		});
		
		return true;
	};

	const AddMessage = (message) => {
		var msg = document.createElement('chat-msg');
		$(msg.shadowRoot).find('#content').text(`${message.user.userName} says ${message.content}`);
		$('#chat-history').append(msg);
		$(msg)[0].scrollIntoView();
	};

	const EnableSend = () => {
		$('#chat-text-send').prop('disabled', false);
		$('#chat-text-input').prop('disabled', false).attr('placeholder', 'Send a message...');
	};

	const DisableSend = () => {
		$('#chat-text-send').prop('disabled', true);
		$('#chat-text-input').prop('disabled', true).attr('placeholder', 'You cannot type in this channel');
	};

	let userId = null;
	const GetUserId = async () => {
		if (userId == null) {
			try {
				userId = await connection.invoke('GetUserId');
			}
			catch (e) {
				console.error(e);
			}
		}
		return userId;
	};

	let currentChannelId = null;
	let availableChannels = [];
	const GetChannels = async () => {
		try {
			availableChannels = await connection.invoke('GetChannels');
			if (availableChannels.length == 0) {
				console.warn('No channels available');
			}
		}
		catch (e) {
			console.error(e);
		}
	};

	const ClearHistory = () => {
		$('#chat-history').html('');
	};

	const GetLastFewMessages = async () => {
		const lastMessages = await connection.invoke('GetLastMessages', currentChannelId);
		for (const message of lastMessages) {
			AddMessage(message);
		}
	};

	const GetChannelInfo = async (channelId) => {
		return await connection.invoke('GetChannel', channelId);
	}

	const GetCurrentChannelInfo = async () => {
		return await GetChannelInfo(await GetCurrentChannel());
	};

	const SetCurrentChannel = async (channelId) => {
		await GetChannels();
		if ($.inArray(channelId, availableChannels) !== -1) {
			console.log('Switching to channel:', channelId);

			if (currentChannelId != null) {
				connection.off(`ReceiveMessage-${currentChannelId}`);
			}

			currentChannelId = channelId;

			ClearHistory();
			await GetLastFewMessages();

			connection.on(`ReceiveMessage-${currentChannelId}`, (msg) => {
				ChatSystem.AddMessage(msg);
			});

			let channel = await GetCurrentChannelInfo();
			$('#chat-header-title').text('Channel: ' + channel.topic);
		}
		else {
			console.warn(`The specified channel ${channelId} does not exist.`);
		}
	};

	const GetCurrentChannel = async () => {
		if (currentChannelId == null) {
			try {
				await GetChannels();
				await SetCurrentChannel(availableChannels[0]);
			}
			catch (e) {
				console.error(e);
			}
		}

		return currentChannelId;
	};

	const UpdateChannelList = async () => {
		if (availableChannels == []) {
			await GetChannels();
		}

		for (const channel of availableChannels) {
			let channelInfo = await GetChannelInfo(channel);

			let el = document.createElement('chat-channel');
			$(el.shadowRoot).find('#channel-link').click(() => {
				SetCurrentChannel(channel);
				SidebarGone();
			});
			$(el.shadowRoot).find('#channel-name').text(channelInfo.topic);
			$('#chat-insert-channels-here').append(el);
		}
	};

	return {
		/* ASYNC */ Send,
		/*       */ AddMessage,
		/*       */ IsSideBarOpen,
		/*       */ ToggleSideBar,
		/*       */ DisableSend,
		/*       */ EnableSend,
		/* ASYNC */ GetUserId,
		/* ASYNC */ GetCurrentChannel,
		/* ASYNC */ SetCurrentChannel,
		/* ASYNC */ GetCurrentChannelInfo,
		/* ASYNC */ GetChannelInfo,
		/* ASYNC */ UpdateChannelList,
	};
})();

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

ChatSystem.DisableSend();

connection.on('ReceiveMessage', function (user, message) {
	ChatSystem.AddMessage(user.userName, message);
});

connection.start().then(async () => {
	console.log('My user id is:', await ChatSystem.GetUserId());

	let currentChannel = await ChatSystem.GetCurrentChannel();
	if (currentChannel != null) {
		ChatSystem.EnableSend();
		await ChatSystem.UpdateChannelList();
	}

	console.log('Current channel is:', currentChannel);
}).catch(function (err) {
    return console.error(err.toString());
});

$('#chat-text-send').click(async () => {
	if (await ChatSystem.Send($('#chat-text-input').val())) {
		$('#chat-text-input').val('');
	}
});

$('#chat-text-input').on('keypress', (e) => {
	if (e.which == KEYCODES.ENTER) {
		$('#chat-text-send').click();
	}
});

$(() => {
	$('#chat-text-input').focus();
})
