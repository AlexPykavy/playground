param(
    [string]
    $AppId,

    [string]
    $AppKey,

    [string]
    $Uri,

    [string]
    $Method = "GET"
)

$encodedUri = [Net.WebUtility]::UrlEncode($Uri)
$timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
$nonce = [guid]::NewGuid().ToString("N")

$stringToSign = "${AppId}${Method}${encodedUri}${timestamp}${nonce}"

$sha256 = [Security.Cryptography.HMACSHA256]::new([Convert]::FromBase64String($AppKey))
$signature = [Convert]::ToBase64String($sha256.ComputeHash([Text.Encoding]::UTF8.GetBytes($stringToSign)))

$auth = "Authorization: Hmac ${AppId}:${signature}:${nonce}:${timestamp}"

"curl.exe -vkL -H `"$auth`" $uri" #| iex
