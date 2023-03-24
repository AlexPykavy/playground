#/bin/sh

[ -z "$1" ] && echo "No hostname argument passed" && exit 1

hostname=$1

[ -d $hostname ] && rm -r $hostname

mkdir $hostname
pushd $hostname

export MSYS_NO_PATHCONV=1 # see https://github.com/openssl/openssl/issues/8795

openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout cert.key -out cert.crt -subj "/CN=${hostname}/O=${hostname}"
openssl pkcs12 -export -out cert.pfx -inkey cert.key -in cert.crt

popd
