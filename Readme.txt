Requisitos para el juego:
- Kinect 2.0
- Unity 2018.2.6f1
- Oculus Go
- Una red local inalámbrica
- KinectStudio

Pasos para ejecutar el juego:
1. Descargarse ambos proyectos SocketClient y SocketServer.
2. Abrir el proyecto SocketServer en Unity.
3. Configurar el entorno Unity y tu PC para Oculus Go:
4. Conectar el Oculus Go en la computadora donde tienes abierto el proyecto SocketServer.
5. Hacer click en File, luego click en Build and run.
6. Abrir el otro proyecto SocketClient.
7. Conectar el Oculus Go y el PC donde está SocketClient a la misma red local.
8. Dentro de la escena KinectAvatars hacer click en el GameObject "SocketClient", luego buscar el componente "SocketClient" y modificar el campo "Server Ip" según el IP local del Oculus Go.
9. Hacer click en Play en el proyecto SocketClient.
10. Abrir KinectStudio y conectar el Kinect a la PC donde este el proyecto "SocketClient".
11. Dentro de KinectStudio hacer click en "Conectar" y luego en PlayFile.
