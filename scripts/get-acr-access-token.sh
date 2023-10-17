#/bin/sh

[ -z "$1" ] && echo "No ACR name argument passed" && exit 1
[ -z "$2" ] && echo "No ACR repository argument passed" && exit 2
[ -z "$3" ] && echo "No ACR actions argument passed (pull|push|pull,push)" && exit 3

registry="$1"
repository="$2"
actions="$3"
scope="repository:$repository:$actions"

aad_access_token=$(az account get-access-token --query accessToken -o tsv)

acr_refresh_token=$(curl -s -X POST -H "Content-Type: application/x-www-form-urlencoded" \
	-d "grant_type=access_token&service=$registry&access_token=$aad_access_token" \
	"https://$registry/oauth2/exchange" \
	| jq -r ".refresh_token")

curl -s -X POST -H "Content-Type: application/x-www-form-urlencoded" \
	-d "grant_type=refresh_token&service=$registry&scope=$scope&refresh_token=$acr_refresh_token" \
	"https://$registry/oauth2/token" \
	| jq -r ".access_token"

# To get manifest by digest
# curl -H "Authorization: bearer $token" \
# 	https://tfhubnetcr.azurecr.io/v2/imsproxy/cdext-api/manifests/sha256:f1b9fa357c3965f3a28dd2f91262cea86e0ef4769f2af57871a6d3238da10239 > manifest.json

# To create manifest
# curl -X PUT \
# 	-H "content-type: application/vnd.docker.distribution.manifest.v2+json" \
# 	-H "Authorization: Bearer $token" \
# 	-d @manifest.json \
# 	https://tfhubnetcr.azurecr.io/v2/imsproxy/cdext-api/manifests/0.2.0-RC.1
