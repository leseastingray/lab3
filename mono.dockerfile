FROM debian:buster-slim

RUN set -x \
    && apt update \
    && apt install -y --no-install-recommends \
       apt-transport-https gnupg1 ca-certificates

RUN set -x \    
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/debian stable-stretch main" > /etc/apt/sources.list.d/mono-official-stable.list

RUN set -x \
    && apt update \
    && apt install mono-complete -y
