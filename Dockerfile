FROM ubuntu:20.04

ENV DEBIAN_FRONTEND noninteractive
RUN apt update && apt upgrade -y
RUN apt install wget software-properties-common git -y

RUN wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-add-repository universe
RUN apt install apt-transport-https -y
RUN apt update
RUN apt install dotnet-sdk-3.1 -y

COPY ./ /root
WORKDIR /root/noc/noc
RUN dotnet build

ENTRYPOINT ["dotnet", "run", "--no-build"]
