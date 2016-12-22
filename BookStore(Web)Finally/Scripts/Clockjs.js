var currentEndAngle = 0,
  currentStartAngle = 0,
  currentColor = '#4A4852',
  lineRadius = 100,
  lineWidth = 10;

var dayNames = new Array('Sunday', 'Monday', 'Tuesday',
  'Wednesday', 'Thursday', 'Friday', 'Saturday');
var months = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

var canvas, context, x, y, now, color = true;

$(document).ready(function () {
    readyCavas();
    setInterval(clock, 1000);
    setInterval(draw, 1000);
    countDown();
})

function clock() {

    now = new Date();
    var outStr = months[now.getMonth()] + ", " + now.getDate();
    $('.clk_days').html(dayNames[now.getDay()]);
    $('.clk_date').html(outStr);
    var outStr = checkTime(now.getHours()) + ':' + checkTime(now.getMinutes()); //+ ':'+now.getSeconds();
    $('.clk_hours').html(outStr);
    if (color) {
        currentEndAngle = 0.034 * now.getSeconds();
        color = false;
    }

}

function checkTime(time) {
    if (time < 10) {
        return '0' + time;
    }
    return time;
}

function readyCavas() {
    canvas = document.getElementById("canvas");
    context = canvas.getContext("2d");
    x = canvas.width / 2;
    y = canvas.height / 2;
}

function draw() {

    var radius;
    var width;

    var startAngle = currentStartAngle * Math.PI;
    var endAngle = currentEndAngle * Math.PI;

    currentStartAngle = currentEndAngle - 0.033333;
    currentEndAngle = currentEndAngle + 0.033333;

    radius = lineRadius;
    width = lineWidth;

    if (Math.floor(currentStartAngle / 2) % 2) {
        currentColor = "#4A4852";
        width = lineWidth + 2;
    } else {
        currentColor = "#72ECB5";
    }

    var counterClockwise = false;
    context.beginPath();
    context.arc(x, y, radius, startAngle, endAngle, counterClockwise);
    context.lineWidth = width;
    context.strokeStyle = currentColor;
    context.stroke();
}

function countDown() {
    // set the date we're counting down to
    var target_date = new Date("dec 31, 2015").getTime();

    // variables for time units
    var days, hours, minutes, seconds;

    // get tag element
    var countdown = document.getElementById("countdown");

    // update the tag with id "countdown" every 1 second
    setInterval(function () {

        // find the amount of "seconds" between now and target
        var current_date = new Date().getTime();
        var seconds_left = (target_date - current_date) / 1000;

        // do some time calculations
        days = parseInt(seconds_left / 86400);
        seconds_left = seconds_left % 86400;

        hours = parseInt(seconds_left / 3600);
        seconds_left = seconds_left % 3600;

        minutes = parseInt(seconds_left / 60);
        seconds = parseInt(seconds_left % 60);

        // countdown.innerHTML = days + "d, " + hours + "h, "
        //+ minutes + "m, " + seconds + "s";   
        $('.days').html(checkTime(days) + ' Days');
        $('.hours').html(checkTime(hours) + ' Hours');
        $('.mins').html(checkTime(minutes) + ' Minutes');
        $('.secs').html(checkTime(seconds) + ' Seconds');

    }, 1000);
}