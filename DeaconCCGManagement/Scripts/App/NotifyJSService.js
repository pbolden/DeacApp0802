/**
 * Uses the NotifyJS library to display notifications on the page.
 * @param  icon
 * @param  type
 * @param  title
 * @param  message
 * @param  url
 * @param  linkText
 * @param  notifyId
 * @param  autoHideDelay 
 */


// Displays the notification on the page.
$(function () {
    $.showNotification = function (icon, type, title, message, url, linkText, notifyId, autoHideDelay) {

        $.notify({
            icon: '<span class=\"' + icon + '\"></span>',
            title: title,
            message: message,
            link: '<a target=\"_blank\" href=\"' + url + '\">' + linkText + '</a>',
            closeX: '<span class=\"glyphicon glyphicon-remove\"></span>',
            notifyId: '<input hidden=\"hidden\" value=\"' + notifyId + '\" />',
        },
            {
                style: 'metro',
                className: type, // info, warning, success, error, black, white
                autoHide: true,
                clickToHide: true,
                autoHideDelay: autoHideDelay,
                position: 'top right',
            }
        );
    }
})(jQuery)


/*
var options = {

    // whether to hide the notification on click
    clickToHide: true,
    // whether to auto-hide the notification
    autoHide: true,
    // if autoHide, hide after milliseconds
    autoHideDelay: 15000,
    // show the arrow pointing at the element
    arrowShow: true,
    // arrow size in pixels
    arrowSize: 5,
    // position defines the notification position though uses the defaults below
    position: 'top right',
    // default positions
    elementPosition: 'top center',
    globalPosition: 'top right',
    // default style
    style: 'bootstrap',
    // default class (string or [string])
    className: 'success',
    // show animation
    showAnimation: 'slideDown',
    // show animation duration
    showDuration: 400,
    // hide animation
    hideAnimation: 'slideUp',
    // hide animation duration
    hideDuration: 200,
    // padding between element and notification
    gap: 2
};
*/