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

	const Send = (input) => {
		if (input == '') {
			return false;
		}

		connection.invoke('SendMessage', '__user__', input).catch(function (err) {
			return console.error(err.toString());
		});
		
		AddMessage(input);
		return true;
	};

	const AddMessage = (input) => {
		var msg = document.createElement('chat-msg');
		$(msg.shadowRoot).find('#content').text(input);
		$('#chat-history').append(msg);
		$(msg)[0].scrollIntoView();
	};

	const EnableSend = () => {
		$('#chat-text-send').prop('disabled', false);
	};

	const DisableSend = () => {
		$('#chat-text-send').prop('disabled', true);
	};

	return { Send, AddMessage, IsSideBarOpen, ToggleSideBar, DisableSend, EnableSend };
})();

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

ChatSystem.DisableSend();

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
	ChatSystem.EnableSend();
}).catch(function (err) {
    return console.error(err.toString());
});

// $('#chat-text-send').click(function (event) {
//     var user = document.getElementById("userInput").value;
//     var message = document.getElementById("messageInput").value;
//     connection.invoke("SendMessage", user, message).catch(function (err) {
//         return console.error(err.toString());
//     });
//     event.preventDefault();
// });

$('#chat-text-send').click(() => {
	if (ChatSystem.Send($('#chat-text-input').val())) {
		$('#chat-text-input').val('');
	}
});

$('#chat-text-input').on('keypress', (e) => {
	if (e.which == KEYCODES.ENTER) {
		$('#chat-text-send').click();
	}
});