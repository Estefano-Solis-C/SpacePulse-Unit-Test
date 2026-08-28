import { IoTDeviceAssembler } from '../../../../src/app/features/iot/assemblers/iot-device.assembler';
import { IoTDeviceSummaryDto, IoTDeviceDetailExtendedDto } from '../../../../src/app/features/iot/models/iot-device.dto';

describe('IoTDeviceAssembler', () => {
  const summaryDto: IoTDeviceSummaryDto = {
    id: 10,
    spaceId: 101,
    type: 'AirConditioning',
    name: 'Master AC Unit',
    serialNumber: 'SN-9948'
  };

  const detailDto: IoTDeviceDetailExtendedDto = {
    id: 10,
    spaceId: 101,
    type: 'AirConditioning',
    name: 'Master AC Unit',
    serialNumber: 'SN-9948',
    metricName: 'Temperature',
    unit: '°C',
    value: 23.5,
    timestamp: '2026-08-27T10:00:00Z',
    isOn: true,
    minThreshold: 18,
    maxThreshold: 26,
    isInAlertState: false
  };

  it('should transform IoTDeviceSummaryDto to IoTDeviceModel', () => {
    const model = IoTDeviceAssembler.toModel(summaryDto);
    expect(model.id).toBe(10);
    expect(model.spaceId).toBe(101);
    expect(model.name).toBe('Master AC Unit');
    expect(model.type).toBe('AirConditioning');
  });

  it('should transform extended telemetry DTO into TelemetryReadingModel with alert states', () => {
    const telemetry = IoTDeviceAssembler.toTelemetryModel(detailDto);
    expect(telemetry.id).toBe(10);
    expect(telemetry.value).toBe(23.5);
    expect(telemetry.unit).toBe('°C');
    expect(telemetry.isInAlertState).toBeFalse();
    expect(telemetry.minThreshold).toBe(18);
    expect(telemetry.maxThreshold).toBe(26);
  });
});
