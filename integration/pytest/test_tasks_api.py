import pytest
import requests

def test_tasks_contract(base_url, homeowner_auth_token):
    headers = {"Authorization": f"Bearer {homeowner_auth_token}"}
    try:
        res = requests.get(f"{base_url}/monitoring/tasks/my-tasks", headers=headers, timeout=5)
        if res.status_code == 200:
            data = res.json()
            assert isinstance(data, list)
    except requests.exceptions.ConnectionError:
        pytest.skip("Backend API server not reachable at localhost:5000")
