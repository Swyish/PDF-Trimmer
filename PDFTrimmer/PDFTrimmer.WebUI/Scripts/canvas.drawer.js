$(document).ready(function () {
    var mouseDown = false;
    var startX = 0;
    var startY = 0;
    var endX = 0;
    var endY = 0;

    //
    // On mouse click, set the start x, y position
    //
    $("#overlay").mousedown(function (e) {

        $("#process-submit").prop('disabled', false);
        $("#marginLeft").prop('disabled', false);
        $("#marginRight").prop('disabled', false);
        $("#marginTop").prop('disabled', false);
        $("#marginBottom").prop('disabled', false);

        mouseDown = true;

        var pos = findPos(this);
        endX = e.pageX - pos.x;
        endY = e.pageY - pos.y;

        var c = document.getElementById("overlay");
        var ctx = c.getContext("2d");

        startX = endX;
        startY = endY;
    });

    //
    // On mouse move, draw the rectangle with start x, y, and mouse position
    //
    $("#overlay").mousemove(function (e) {
        if (mouseDown) {

            var c = document.getElementById("overlay");
            var ctx = c.getContext("2d");

            ctx.clearRect(0, 0, canvas.width, canvas.height);

            var pos = findPos(this);
            endX = e.pageX - pos.x;
            endY = e.pageY - pos.y;

            ctx.beginPath();
            ctx.fillStyle = "rgba(0, 0, 0, 0.2)";
            ctx.fillRect(startX, startY, endX - startX, endY - startY)

            if (startY > endY) {
                // Handling starting from bototm left

                $("#marginBottom").val(canvas.height - startY);

                $("#marginTop").val(endY);
            } else {
                // Handling starting from bototm left
                $("#marginBottom").val(canvas.height - endY);
                $("#marginTop").val(startY);
            }

            if (startX > endX) {
                $("#marginLeft").val(endX);
                $("#marginRight").val(canvas.width - startX);

            } else {
                $("#marginLeft").val(startX);
                $("#marginRight").val(canvas.width - endX);
            }
        }
    });

    //
    // Mouse up will stop drawing the rectangle
    //
    $("#overlay").mouseup(function (e) {
        mouseDown = false;
    });

    //
    // Handles the manual margin changes from the marginLeft textbox
    //
    $("#marginLeft").change(function () {
        if (startX > endX) {
            endX = $("#marginLeft").val();
        } else {
            startX = $("#marginLeft").val();
        }
        redrawOverlay();
    });

    //
    // Handles the manual margin changes from the marginBottom textbox
    //
    $("#marginBottom").change(function () {
        if (endY > startY) {
            endY = canvas.height - $("#marginBottom").val();
        } else {
            startY = canvas.height - $("#marginBottom").val();
        }
        redrawOverlay();
    });

    //
    // Handles the manual margin changes from the marginRight textbox
    //
    $("#marginRight").change(function () {
        if (startX > endX) {
            startX = canvas.width - $("#marginRight").val();
        } else {
            endX = canvas.width - $("#marginRight").val();
        }
        redrawOverlay();
    });

    //
    // Handles the manual margin changes from the marginTop textbox
    //
    $("#marginTop").change(function () {
        if (endY > startY) {
            startY = $("#marginTop").val();
        } else {
            endY = $("#marginTop").val();
        }
        redrawOverlay();
    });

    $("#process-submit").click(function () {
        setTimeout(function () {
            window.location.href = "/thankyou";
        }, 10000);
    });

    //
    // Redraw canvas overlay
    //
    function redrawOverlay() {
        var c = document.getElementById("overlay");
        var ctx = c.getContext("2d");

        ctx.clearRect(0, 0, canvas.width, canvas.height);

        ctx.beginPath();
        ctx.fillStyle = "rgba(0, 0, 0, 0.2)";
        ctx.fillRect(startX, startY, endX - startX, endY - startY)
    }

    //
    // Find position of the mouse in the obj
    //
    function findPos(obj) {
        var curleft = 0, curtop = 0;
        if (obj.offsetParent) {
            do {
                curleft += obj.offsetLeft;
                curtop += obj.offsetTop;
            } while (obj = obj.offsetParent);
            return { x: curleft, y: curtop };
        }
        return undefined;
    }
});