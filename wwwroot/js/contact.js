function setRecaptchaResponse() {
	let response = grecaptcha.getResponse();
	let el = document.getElementById('g-recaptcha-response');
	el.value = response;
	el.name = 'Captcha';

	return response !== "";
}

$(() => {
	$('#contact').on('submit', (event) => {
		setRecaptchaResponse());
	});
})