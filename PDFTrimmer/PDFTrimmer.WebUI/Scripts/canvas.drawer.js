$(document).ready(function () {
    var mouseDown = false;
    var startX = 0;
    var startY = 0;

    $("#overlay").mousedown(function (e) {
        mouseDown = true;

        var pos = findPos(this);
        var x = e.pageX - pos.x;
        var y = e.pageY - pos.y;

        var c = document.getElementById("overlay");
        var ctx = c.getContext("2d");

        startX = x;
        startY = y;

        // ctx.rect(x, y, x, y);
        // ctx.stroke();


    });

    $("#overlay").mousemove(function (e) {
        if (mouseDown) {
            var c = document.getElementById("overlay");
            var ctx = c.getContext("2d");

            ctx.clearRect(0, 0, canvas.width, canvas.height);

            var pos = findPos(this);
            var x = e.pageX - pos.x;
            var y = e.pageY - pos.y;

            ctx.beginPath();
            ctx.fillStyle = "rgba(0, 0, 0, 0.2)";
            ctx.fillRect(startX, startY, x - startX, y - startY)

            if (startY > y) {
                // Handling starting from bototm left
                $("#marginLeft").val(startX);
                $("#marginBottom").val(canvas.height - startY);
                $("#marginRight").val(canvas.width - x);
                $("#marginTop").val(y);
            } else {
                // Handling starting from bototm left
                $("#marginLeft").val(startX);
                $("#marginBottom").val(canvas.height - y);
                $("#marginRight").val(canvas.width - x);
                $("#marginTop").val(startY);
            }
        }
    });

    $("#overlay").mouseup(function (e) {
        mouseDown = false;
        startX = 0;
        startY = 0;

        //var pos = findPos(this);
        //var x = e.pageX - pos.x;
        //var y = e.pageY - pos.y;

        //var c = document.getElementById("overlay");
        //var ctx = c.getContext("2d");

        //ctx.clearRect(0, 0, canvas.width, canvas.height);
    });

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