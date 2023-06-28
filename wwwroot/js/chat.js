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

	const RemoveMessage = (id) => {
		$(`chat-msg[msg-id=${id}]`).replaceWith('');
	};

	const AddMessage = (message) => {
		var msg = document.createElement('chat-msg');
		$(msg).attr('msg-id', message.id);

		let sr = $(msg.shadowRoot);
		sr.find('#user').text(message.user.userName);
		sr.find('#content').text(message.content);
		sr.find('#timestamp').text(new Date(message.timestamp).toLocaleString());
		sr.find('#view-options').click(() => {
			$('#phone-context-menu-bg').attr('show', '');
			$('#phone-context-menu').attr('show', '');
			$('#phone-context-menu').attr('msg', JSON.stringify(message));
		});

		sr.find('#main').dblclick((e) => {
			sr.find('#view-options').click();
			e.preventDefault();
		})

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
		if (channelId == currentChannelId) {
			return; // No.
		}

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
			await UpdateChannelList();
		}

		$('#chat-insert-channels-here').html('');
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

	const CreateChannel = async (topic) => {
		try {
			let id = await connection.invoke('CreateChannel', await GetUserId(), topic);
			await SetCurrentChannel(id);
			await UpdateChannelList();
			SidebarGone();
		}
		catch (e) {
			console.error(e);
		}
	};

	const DeleteMessage = async (id) => {
		try {
			await connection.invoke('DeleteMessage', await GetUserId(), id);
		}
		catch (e) {
			console.error(e);
		}
	};

	return {
		/* ASYNC */ Send,
		/*       */ AddMessage,
		/*       */ RemoveMessage,
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
		/* ASYNC */ CreateChannel,
		/* ASYNC */ DeleteMessage,
	};
})();

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

ChatSystem.DisableSend();

connection.on('ReceiveMessage', function (user, message) {
	ChatSystem.AddMessage(user.userName, message);
});

connection.on('DeleteMessage', (id) => {
	ChatSystem.RemoveMessage(id);
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

$('#chat-create-channel').click(() => {
	let channelTopic = prompt('Channel topic?', 'C++ is better than Rust');
	if (channelTopic.length > 36) {
		alert('The channel topic is too long. Must be less than 36 characters.');
		return;
	}

	ChatSystem.CreateChannel(channelTopic);
});

$(() => {
	$('#chat-text-input').focus();
})

const CopyToClipboard = (value) => {
	var $temp = $("<input>");
	$("body").append($temp);
	$temp.val(value).select();
	document.execCommand("copy");
	$temp.remove();
};

const CtxMenu = (() => {
	const Hide = () => {
		$('#phone-context-menu-bg').removeAttr('show');
		$('#phone-context-menu').removeAttr('show');
	};

	const GetMsg = () => {
		return JSON.parse($('#phone-context-menu').attr('msg'));
	};

	return {
		Hide,
		GetMsg
	};
})();

$('#phone-context-menu-bg').click(() => {
	CtxMenu.Hide();
})

$('#delete-message').click(() => {
	let msg = CtxMenu.GetMsg();
	ChatSystem.DeleteMessage(msg.id);
	CtxMenu.Hide();
})

$('#copy-channel-id').click(() => {
	let msg = CtxMenu.GetMsg();
	CopyToClipboard(msg.channelId);
	CtxMenu.Hide();
})

$('#copy-message-id').click(() => {
	let msg = CtxMenu.GetMsg();
	CopyToClipboard(msg.id);
	CtxMenu.Hide();
})

$('#copy-user-id').click(() => {
	let msg = CtxMenu.GetMsg();
	CopyToClipboard(msg.userId);
	CtxMenu.Hide();
})
