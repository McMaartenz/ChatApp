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

// Bind click handlers
$('#cookie-bar #accept').click((e) => {
    $('#cookie-bar').fadeOut();
    setCookie('gdpr', 'accept', 365);
});

$('#cookie-bar #refuse').click((e) => { 
    $('#cookie-bar').fadeOut();
    setCookie('gdpr', 'refuse', 1);
});
