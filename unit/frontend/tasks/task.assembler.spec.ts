import { TaskAssembler } from '../../../../src/app/features/tasks/assemblers/task.assembler';
import { WorkItemDto } from '../../../../src/app/features/tasks/models/task.dto';

describe('TaskAssembler', () => {
  const sampleTaskDto: WorkItemDto = {
    id: 1,
    spaceId: 100,
    createdByUserId: 'usr-owner',
    title: 'Replace HVAC Filter',
    description: 'Filter replacement in office unit',
    price: 180,
    status: 'IN_PROGRESS',
    createdAt: '2026-08-27T00:00:00Z',
    plannedStartDate: '2026-08-28',
    plannedEndDate: '2026-08-29'
  };

  it('should transform WorkItemDto into rich TaskModel', () => {
    const model = TaskAssembler.toModel(sampleTaskDto);
    expect(model.id).toBe(1);
    expect(model.title).toBe('Replace HVAC Filter');
    expect(model.status).toBe('IN_PROGRESS');
    expect(model.price).toBe(180);
  });
});
