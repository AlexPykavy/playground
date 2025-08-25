import json
import pytest
import requests
import time
from azure.storage.queue import BinaryBase64DecodePolicy, BinaryBase64EncodePolicy, QueueClient

function_url = "https://pykavy-test.azurewebsites.net"
function_admin_key = "<FUNCTION_ADMIN_KEY>"
connection_string = "<SA_CONNECTION_STRING>"

@pytest.mark.parametrize("execution_number", range(1))
def test_run_multiple_times(execution_number):
    base64_queue_client = QueueClient.from_connection_string(
                                conn_str=connection_string, queue_name="samples",
                                message_encode_policy = BinaryBase64EncodePolicy(),
                                message_decode_policy = BinaryBase64DecodePolicy()
                            )

    message_count_before_test = base64_queue_client.get_queue_properties().approximate_message_count
    base64_queue_client.send_message(bytes(str(time.time()), encoding="utf-8"))

    time.sleep(30)

    requests.post(f"{function_url}/admin/host/drain?code={function_admin_key}")

    while json.loads(requests.get(f"{function_url}/admin/host/drain/status?code={function_admin_key}").content)["state"] == "InProgress":
        time.sleep(5)

    time.sleep(10)

    message_count_after_test = base64_queue_client.get_queue_properties().approximate_message_count

    requests.post(f"{function_url}/admin/host/resume?code={function_admin_key}")

    assert message_count_before_test == message_count_after_test
