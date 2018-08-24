/*
 * Uses SignalR for push notifications.
 * Creates a persistent bi-directional connection from client to server
 * using SignalR, which is an abstraction over web sockets. 
 */

$(function () {

    // TODO: turn off for production
    // $.connection.hub.logging = true;

    var iconInfo = "glyphicon glyphicon-info-sign";
    var iconWarning = "glyphicon glyphicon-warning-sign";
    var iconSuccess = "glyphicon glyphicon-thumbs-up";
    var iconFailure = "glyphicon glyphicon-thumbs-down";

    var icons = [iconInfo, iconWarning, iconSuccess, iconFailure];

    //info, warning, success, error, white, black,
    var typeInfo = "info";
    var typeWarning = "warning";
    var typeSuccess = "success";
    var typeFailure = "error";
    var typeWhite = "white";
    var typeBlack = "black";

    var types = [typeInfo, typeWarning, typeSuccess, typeFailure, typeWhite, typeBlack];

    var isConnected = false;

    // how long before auto-hide notification in milliseconds
    var autoHideDelay = 15000;

    // to prevent user from re-clicking the notify bell
    var lockNotifyBell = false;

    // Declare a proxy to reference the hub.
    var notifyHub = $.connection.notifyHub;

    // Gets user id from hidden element in view
    var userId = $("#userId").attr("value");

    // Called from hub on server to add notification to view
    notifyHub.client.addNotification = function (json) {

        var jsonResult = JSON.parse(json);

        // int used to get items from icon and type arrays
        var typeIndex = jsonResult.type;

        // Determines the icon and color used for notification.
        var type;

        // Use 'white' template if type is 'info' and has a link. 
        if (typeIndex === 0 && jsonResult.url !== "" && jsonResult.url !== null) {
            type = types[4] // "white"
        } else {
            type = types[typeIndex];
        }

        var iconInfo = icons[typeIndex];
        var title = jsonResult.title;
        var message = jsonResult.message;
        var url = jsonResult.url;
        var linkText = jsonResult.linkText;
        var notifyId = jsonResult.notificationId;

        $.showNotification(iconInfo,
            type, title, message, url, linkText, notifyId, autoHideDelay);
    };

    // invoked when notification bell clicked
    $("#notify-link").on("click", function () {
        // Invoked on server hub to send notifications to page      
        if (lockNotifyBell === false) {
            notifyHub.server.sendNotification();
            lockNotifyBell = true;
        }
        // lock notify bell for time of auto-hide delay
        setTimeout(function () {
            lockNotifyBell = false;
        }, autoHideDelay);
    });

    // updates the count in the notification badge
    notifyHub.client.updateNotificationsCount = function (count) {

        if (count === 0) {
            $("#notify-count").text("");
            $("#notify-bell").attr("class", "glyphicon glyphicon-bell notify-inactive");
        } else {
            $("#notify-count").text(count);
            $("#notify-bell").attr("class", "glyphicon glyphicon-bell notify-active");
        }
    };

    // on click listener for the notification base class
    // Triggered when user clicks on the notification
    $(document).on('click', '.notifyjs-metro-base', function () {

        // last child element is div; its last child is hidden input with id
        var id = $(this.lastChild.lastChild).attr("value");
        notifyHub.server.removeNotification(id);
        notifyHub.server.getNotificationsCount();
    });

    // Updates notifications count every X milliseconds
    var ticks = 0;
    var interval = 60000;
    var startUpdateCountInterval = function () {
        setInterval(function () {
            if (isConnected === true) {
                notifyHub.server.getNotificationsCount();
                //console.log("tick", ticks++);
            } else { // stop            
                clearInterval(startUpdateCountInterval);
            }

        }, interval);
    }



    // Uses JS Storage to store a boolean flag in the browser cache.
    // If false, call method on server to notify user of special dates
    // like members birthdays. 
    // storage session uses key/value pairs
    var key = "notifiedOfSpecialDates";
    var storage = Storages.sessionStorage;
    var notifyUserOfSpecialDates = function () {
        var notifiedOfSpecialDates = storage.get(key);

        //console.log("notifiedOfSpecialDates", notifiedOfSpecialDates);

        if (notifiedOfSpecialDates !== true) {
            notifyHub.server.notifyOfMembersSpecialDates();
            storage.set(key, "true");

            //colsole.log("notifiedOfSpecialDates", notifiedOfSpecialDates + " does not equal true");                       
        }
    }

    // Start the persistent signalR connection.
    $.connection.hub.start()
        .done(function () {
            //console.log("connected!");
            isConnected = true;
            notifyUserOfSpecialDates();
            notifyHub.server.getNotificationsCount();
            startUpdateCountInterval();
        })
        .fail(function () {
            //console.log("not connected!");
            isConnected = false;
        });
});