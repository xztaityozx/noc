FROM ubuntu:18.04
MAINTAINER xztaityozx

RUN apt update && apt upgrade -y
RUN apt install wget software-properties-common git -y


RUN wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-add-repository universe
RUN apt install apt-transport-https -y
RUN apt update
RUN apt install dotnet-sdk-2.1 -y

WORKDIR ~/
RUN git clone https://github.com/xztaityozx/noc
WORKDIR noc/noc/noc
RUN dotnet build

CMD ["dotnet","run", "--no-build"]
