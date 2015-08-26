Bingle Image Stitching Server
---
- 폴더 설정방법
>> - [./BingleServer/Debug]
>> - [./AutopanoServer]

- BingleServer 실행 방법(using SuperSocket)
>> notice : app.config와 SuperSocket.SocketService.exe.config에 모두 설정해 주어야 함.
>> 1. ./BingleServer.exe(app.config)
>> 2. using mono, mono-service(SuperSocket.SocketService.exe.config)

- Autopano Server 위치 : ./AutopanoServer
>> notice : AutopanoServer/README 참조

- 문제점/개선사항
>> 1. Autopano Server 실행시간으로 인한 서버 부하 : GPU 이용하기
>> 2. 패킷 밀림 현상이 간혹 발생함
>> 3. 파일 관리를 위한 로직이 필요함.
>> 4. 속도 향상을 위한 Application Protocol 간결화