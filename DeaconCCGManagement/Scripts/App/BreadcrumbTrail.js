/*
 * Implements the plug-in JS Storage to get/set breadcrumbs in the
 * browser's session storage and displays them in views.
 */

$(function () {

    // storage session uses key/value pairs
    var key = "breadcrumbs";

    // will hold an array of breadcrumb objects
    var breadcrumbs;

    // max number of breadcrumbs that will display on page
    var maxBreadcrumbs = 6;

    // character that separates breadcrumbs
    var separator = "&nbsp;&nbsp;&raquo;";
   
    // session storage clears when browser window closes
    var storage = Storages.sessionStorage;

    // get url for current page
    var url = document.URL;

    // get link text for current page
    var linkText = $("#breadcrumb_link_text").text();

    if (linkText === null || linkText === "") { return; }

    // writes breadcrumbs to JS console (for debugging only)
    //var logCrumbs = function () {
    //    var crumbs = "";
    //    for (var i = 0; i < breadcrumbs.length; i++) {
    //        crumbs = crumbs + breadcrumbs[i].linkText + "|" + breadcrumbs[i].url + "\n";
    //    }
    //    console.log(crumbs);
    //}

    // checks if breadcrumb already stored; returns index if yes, -1 if no
    var breadcrumbInStorage = function(crumb, crumbs) {
        for (var i = 0; i < crumbs.length; i++) {
            // compare link text
            if (crumb.linkText === crumbs[i].linkText) {
                return crumbs.indexOf(crumbs[i]);
            }
        }
        return crumbs.indexOf(crumb);
    }

    var updateBreadcrumbs = function(index, breadcrumb) {
        // remove breadcrumb and all breadcrumbs to right of it 
        var start = index;
        var deleteCount = breadcrumbs.length - start;
        breadcrumbs.splice(start, deleteCount);

        // push updated breadcrumb. this ensures the url gets updated.
        breadcrumbs.push(breadcrumb);
    }

    // truncates array if length > maxBreadcrumbs
    var truncateBreadcrumbs = function() {
        // get last maxBreadcrumbs
        var start = breadcrumbs.length - maxBreadcrumbs;
        var deleteCount = maxBreadcrumbs;
        breadcrumbs = breadcrumbs.splice(start, deleteCount);
    }

    var getBreadcrumbLink = function (breadcrumb) {
        return "<a href=\"" + breadcrumb.url + "\">" + breadcrumb.linkText + "</a>";
    }
    
    // if storage empty, create empty array
    if (storage.isEmpty(key)) {
        breadcrumbs = [];
    } else {
        // get breadcrumb array from storage
        breadcrumbs = storage.get(key);
    }

    // create breadcrumb object
    var breadcrumb = { linkText: linkText, url: url };

    // check if current page already stored (returns -1 if not)
    // $.inArray does not work here since it checks memory location
    var index = breadcrumbInStorage(breadcrumb, breadcrumbs);

    // if index doesn't equal -1, breadcrumb already exists in array
    if (index !== -1) {
        updateBreadcrumbs(index, breadcrumb);
    } else {
        breadcrumbs.push(breadcrumb);
    }

    // store updated breadcrumbs in browser
    storage.set(key, breadcrumbs);

    //
    // DEBUG
    //
    //console.log("breadcrumbs length: " + breadcrumbs.length);
    //logCrumbs();
    //console.log(storage.get(key));

    //
    // display breadcrumbs on page
    //

    // truncate breadcrumbs  
    if (breadcrumbs.length > maxBreadcrumbs) {
        $("#breadcrumbs").append("<li>...</li>");
        $("#breadcrumbs").append(separator);
        truncateBreadcrumbs();
    }

    var spanSeparator = "<span class=\"divider\">" + separator + "</span>";
    // write breadcrumb list items to page
    for (var j = 0; j < breadcrumbs.length; j++) {
        if (j !== breadcrumbs.length - 1) {
            $("#breadcrumbs").append("<li>" + getBreadcrumbLink(breadcrumbs[j]) + spanSeparator + "</li>");
        } else {
            $("#breadcrumbs").append("<li class=\"active\">" + breadcrumbs[j].linkText + "</li>");
        }
    }
});