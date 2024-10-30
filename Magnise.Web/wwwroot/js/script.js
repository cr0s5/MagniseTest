let socket = null;

document.getElementById("openButton").onclick = function () {
    var instrumentValue = document.getElementById('instrument-id').value;

    socket = new WebSocket("https://localhost:5000/ws/" + instrumentValue);

    socket.onopen = function () {
        console.log("Connected to WebSocket server");
        var msg = { type: "start" };
        socket.send(JSON.stringify(msg));
        console.log("Sent start");
    };

    socket.onmessage = function (event) {
        document.getElementById("response").innerText += "Message from server: " + event.data + "\n";
    };
};

document.getElementById("closeButton").onclick = function () {
    if (socket.readyState === WebSocket.OPEN) {
        var msg = { type: "end" };
        socket.send(JSON.stringify(msg));
        socket.close();
        document.getElementById("response").innerText = "Disconnected from server.";
    } else {
        document.getElementById("response").innerText = "WebSocket is already closed or not connected.";
    }
};

document.getElementById("subscribeButton").onclick = function () {
    if (socket.readyState === WebSocket.OPEN) {
        var instrumentValue = document.getElementById('instrument-id').value;
        var msg = { type: "subscribe", id: instrumentValue };
        socket.send(JSON.stringify(msg));
    }
};