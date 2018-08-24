$.notify.addStyle("metro", {
    html:
        "<div>" +
        "<div class='closeX' data-notify-html='closeX'/>" + 
       "<div class='icon' data-notify-html='icon'/>" +        
            "<div class='text-wrapper'>" +
                "<div class='title' data-notify-html='title'/>" +
                "<div class='message' data-notify-html='message'/>" +
                "<div class='url' data-notify-html='link'/>" +
        "</div>" +
        "<div data-notify-html='notifyId'/>" +
        "</div>",
    classes: {
        error: {
            "color": "#fafafa !important",
            "background-color": "#F71919",
            "border": "1px solid #FF0026"
        },
        success: {
            "background-color": "#32CD32",
            "border": "1px solid #4DB149"
        },
        info: {
            "color": "#fafafa !important",
            "background-color": "#1E90FF",
            "border": "1px solid #1E90FF"
        },
        warning: {
            "background-color": "#FAFA47",
            "border": "1px solid #EEEE45"
        },
        black: {
            "color": "#fafafa !important",
            "background-color": "#333",
            "border": "1px solid #000"
        },
        white: {
            "background-color": "#f1f1f1",
            "border": "1px solid #ddd"
        }
    }
});