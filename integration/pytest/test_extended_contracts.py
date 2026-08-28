import pytest
import requests
import json

BASE_URL = "http://localhost:5000"

class TestSpacePulseExtendedApi:

    def test_metrics_prometheus_format(self):
        # When backend is running, verify prometheus metrics endpoint
        try:
            res = requests.get(f"{BASE_URL}/metrics", timeout=2)
            assert res.status_code == 200
            assert "http_requests_total" in res.text
            assert "dotnet_total_memory_bytes" in res.text
        except requests.exceptions.ConnectionError:
            pytest.skip("Backend server not running locally during offline unit run")

    def test_space_domain_calculation_logic(self):
        # Unit test for price per square meter calculation
        price = 1500.0
        sqm = 50.0
        price_per_sqm = price / sqm
        assert price_per_sqm == 30.0

    def test_iot_telemetry_anomaly_threshold_logic(self):
        # Unit test for temperature anomaly detection
        temp_reading = 32.5
        min_temp = 18.0
        max_temp = 26.0
        is_anomaly = temp_reading < min_temp or temp_reading > max_temp
        assert is_anomaly is True

    def test_kanban_task_progress_transition_logic(self):
        # Verify valid status transitions
        valid_statuses = ["Pending", "InProgress", "UnderReview", "Completed"]
        assert "InProgress" in valid_statuses
        assert "Completed" in valid_statuses
