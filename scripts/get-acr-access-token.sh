#/bin/sh

[ -z "$1" ] && echo "No ACR name argument passed" && exit 1
[ -z "$2" ] && echo "No ACR repository argument passed" && exit 2

registry=$1
repository=$2
scope="repository:$repository:pull"

aad_access_token=$(az account get-access-token --query accessToken -o tsv)

acr_refresh_token=$(curl -s -X POST -H "Content-Type: application/x-www-form-urlencoded" \
	-d "grant_type=access_token&service=$registry&access_token=$aad_access_token" \
	https://$registry/oauth2/exchange \
	| jq -r '.refresh_token')

curl -s -X POST -H "Content-Type: application/x-www-form-urlencoded" \
	-d "grant_type=refresh_token&service=$registry&scope=$scope&refresh_token=$acr_refresh_token" \
	https://$registry/oauth2/token \
	| jq -r '.access_token'
