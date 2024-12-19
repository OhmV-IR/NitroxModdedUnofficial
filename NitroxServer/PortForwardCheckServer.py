import socket


sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind(("127.0.0.1", 7000))

while True:
    # Receive data from the socket
    data, addr = sock.recvfrom(1024)
    print("Received data from socket:")
    print(data)