import { NotificationAssembler } from '../../../../src/app/features/notifications/assemblers/notification.assembler';
import { NotificationDto } from '../../../../src/app/features/notifications/models/notification.dto';

describe('NotificationAssembler', () => {
  const sampleDto: NotificationDto = {
    id: 50,
    spaceId: 101,
    title: 'Critical IoT Alert',
    message: 'High temperature anomaly detected',
    isRead: false,
    createdAt: '2026-08-27T12:00:00Z'
  };

  it('should transform NotificationDto to NotificationModel', () => {
    const model = NotificationAssembler.toModel(sampleDto);
    expect(model.id).toBe(50);
    expect(model.title).toBe('Critical IoT Alert');
    expect(model.isRead).toBeFalse();
  });
});
