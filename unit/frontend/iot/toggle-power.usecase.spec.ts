import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { TogglePowerUseCase } from '../../../../src/app/features/iot/application/use-cases/toggle-power.usecase';
import { IOT_REPOSITORY_TOKEN } from '../../../../src/app/features/iot/domain/repositories/iot.tokens';
import { IoTRepository } from '../../../../src/app/features/iot/domain/repositories/iot.repository';
import { TogglePowerResponseDto } from '../../../../src/app/features/iot/models/iot-device.dto';

describe('TogglePowerUseCase', () => {
  let useCase: TogglePowerUseCase;
  let mockRepository: jasmine.SpyObj<IoTRepository>;

  const mockResponse: TogglePowerResponseDto = {
    message: 'Dispositivo encendido exitosamente.',
    isOn: true,
    data: {
      id: 5,
      spaceId: 101,
      type: 'Lighting',
      name: 'Smart Light 1',
      serialNumber: 'SN-001',
      isOn: true
    }
  };

  beforeEach(() => {
    mockRepository = jasmine.createSpyObj<IoTRepository>('IoTRepository', ['togglePower']);

    TestBed.configureTestingModule({
      providers: [
        TogglePowerUseCase,
        { provide: IOT_REPOSITORY_TOKEN, useValue: mockRepository }
      ]
    });

    useCase = TestBed.inject(TogglePowerUseCase);
  });

  it('should call repository togglePower and emit response', (done) => {
    mockRepository.togglePower.and.returnValue(of(mockResponse));

    useCase.execute(5).subscribe({
      next: (res) => {
        expect(res.isOn).toBeTrue();
        expect(res.data.id).toBe(5);
        expect(mockRepository.togglePower).toHaveBeenCalledWith(5);
        done();
      },
      error: done.fail
    });
  });

  it('should handle errors when device is unauthorized or not found', (done) => {
    mockRepository.togglePower.and.returnValue(throwError(() => new Error('Forbidden')));

    useCase.execute(99).subscribe({
      next: () => done.fail('Expected error, got success'),
      error: (err) => {
        expect(err.message).toBe('Forbidden');
        done();
      }
    });
  });
});
