// Counts the remaining characters for an input field.
// Updates the count as the user types in the field.

$(document).ready(function () {

    // find and store the count readout
    var $charCount = $("#charCount", this);
    var $input = $("#message", this);

    // .text() returns a string, multiply by 1 to make it a number (for math)
    var maximumCount = $charCount.text() * 1;

    // update function is called on keyup, paste and input events
    var update = function () {
        var before = $charCount.text() * 1;
        var now = maximumCount - $input.val().length;

        // check to make sure users haven't exceeded their limit
        if (now < 0) {
            var str = $input.val();
            $input.val(str.substr(0, maximumCount));
            now = 0;
        }
        // only alter the DOM if necessary
        if (before !== now) {
            $charCount.text(now);
        }
    };
    // listen for change
    $input.bind("input keyup paste", function () { setTimeout(update, 0) });

    // call update initially, in case input is pre-filled
    update();
});