# Crypto Chat
This application demonstrates RSA-, AES-Encryption and digital signatures, by having a chat application that sends encrypted messages between users.

## Run application

To run the application, you need to first start the server by running in the chatServer directory:
```console
dotnet run
```

To run a client, go into chatClient and run:
```console
dotnet run <your-username>
```

You will then be prompted to type in the user you want to send a message to, and to type the message. After sending, the message will be displayed in the receiver's console window.

For demonstration purposes, incoming messages in the server are written to the console. This makes it possible to see the encrypted messages that are sent to the server.

