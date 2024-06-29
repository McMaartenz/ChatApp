function setRecaptchaResponse() {
	let response = grecaptcha.getResponse();
	let el = document.getElementById('g-recaptcha-response');
	el.value = response;
	el.name = 'Captcha';

	return response !== "";
}

$(() => {
	$('input#topic').val('@(Model?.Topic ?? "")');
	$('input#email').val('@email');
	$('textarea#message').text('@(Model?.Message ?? "")');
	$('#contact').on('submit', (event) => {
		if (!setRecaptchaResponse()) {
			event.preventDefault();
		}
	});
})