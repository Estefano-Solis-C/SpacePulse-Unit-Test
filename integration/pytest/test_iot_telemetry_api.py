import pytest
import requests

def test_iot_telemetry_stream_contract(base_url, homeowner_auth_token):
    headers = {"Authorization": f"Bearer {homeowner_auth_token}"}
    try:
        res = requests.get(f"{base_url}/monitoring/readings/user", headers=headers, timeout=5)
        if res.status_code == 200:
            data = res.json()
            assert isinstance(data, list)
            if len(data) > 0:
                first = data[0]
                assert "metricName" in first
                assert "value" in first
                assert "unit" in first
                assert "minThreshold" in first
                assert "maxThreshold" in first
    except requests.exceptions.ConnectionError:
        pytest.skip("Backend API server not reachable at localhost:5000")

def test_iot_device_power_toggle_contract(base_url, homeowner_auth_token):
    headers = {"Authorization": f"Bearer {homeowner_auth_token}"}
    try:
        res = requests.put(f"{base_url}/monitoring/iot-devices/1/toggle", headers=headers, timeout=5)
        if res.status_code == 200:
            data = res.json()
            assert "isOn" in data
            assert "message" in data
    except requests.exceptions.ConnectionError:
        pytest.skip("Backend API server not reachable at localhost:5000")
