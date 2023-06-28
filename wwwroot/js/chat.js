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
		$("#chat-sidebar").css('left', `${isSideBarOpen * -100}%`);
	};

	const Send = async (input) => {
		if (input == '') {
			return false;
		}

		connection.invoke('SendMessage', await ChatSystem.GetUserId(), input).catch(function (err) {
			return console.error(err.toString());
		});
		
		return true;
	};

	const AddMessage = (user, input) => {
		var msg = document.createElement('chat-msg');
		$(msg.shadowRoot).find('#content').text(`${user} says ${input}`);
		$('#chat-history').append(msg);
		$(msg)[0].scrollIntoView();
	};

	const EnableSend = () => {
		$('#chat-text-send').prop('disabled', false);
	};

	const DisableSend = () => {
		$('#chat-text-send').prop('disabled', true);
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

	return { Send, AddMessage, IsSideBarOpen, ToggleSideBar, DisableSend, EnableSend, GetUserId };
})();

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

ChatSystem.DisableSend();

connection.on('ReceiveMessage', function (user, message) {
	ChatSystem.AddMessage(user.userName, message);
});

connection.start().then(async () => {
	ChatSystem.EnableSend();
	console.log('My user id is: ', await ChatSystem.GetUserId());
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