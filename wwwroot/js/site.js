// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Utility
const setCookie = (name, value, days) => {
    var expires = '';
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = '; expires=' + date.toUTCString();
    }

    document.cookie = `${name}=${value || ''}${expires}; path=/`;
};

const getCookieValue = (name) => (
    document.cookie.match('(^|;)\\s*' + name + '\\s*=\\s*([^;]+)')?.pop() || ''
);

// Test whether cookies are already accepted
const gdpr = getCookieValue('gdpr');
const useCookies = gdpr == 'accept';

if (useCookies) {
    $('#cookie-bar').hide();
}

// GDPR functions
const updateSetting = (value) => {
    $('#current-gdpr-setting').html('Your current setting is: ' + value);
};

const accept = (_) => {
    setCookie('gdpr', 'accept', 365);
    $('#cookie-bar').fadeOut();
    updateSetting('accept');
};

const refuse = (_) => {
    setCookie('gdpr', 'refuse', 1);
    $('#cookie-bar').fadeOut();
    updateSetting('refuse');
};

// Bind click handlers
$('#cookie-bar #accept').click(accept);
$('#cookie-bar #refuse').click(refuse);

$('#change-gdpr #accept').click(accept);
$('#change-gdpr #refuse').click(refuse);

$('#change-gdpr div').hide();
$('#update-gdpr').click((_) => {
    $('#change-gdpr div').fadeToggle();
})
