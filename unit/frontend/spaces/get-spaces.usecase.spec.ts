import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { GetSpacesUseCase } from '../../../../src/app/features/spaces/application/use-cases/get-spaces.usecase';
import { SPACE_REPOSITORY_TOKEN } from '../../../../src/app/features/spaces/domain/repositories/space.tokens';
import { SpaceRepository } from '../../../../src/app/features/spaces/domain/repositories/space.repository';
import { SpaceModel } from '../../../../src/app/features/spaces/models/space.model';

describe('GetSpacesUseCase', () => {
  let useCase: GetSpacesUseCase;
  let mockRepository: jasmine.SpyObj<SpaceRepository>;

  const mockSpaces: SpaceModel[] = [
    {
      id: 'sp-101',
      title: 'Miraflores Office Hub',
      description: 'Smart office with IoT monitoring',
      type: 'Office',
      status: 'Published',
      pricePerMonth: 2500,
      totalPricing: 2500,
      location: {
        address: 'Av. Larco 400',
        city: 'Lima',
        country: 'Peru',
        latitude: -12.122,
        longitude: -77.028
      },
      images: ['https://example.com/img1.jpg'],
      ownerId: 'usr-001'
    }
  ];

  beforeEach(() => {
    mockRepository = jasmine.createSpyObj<SpaceRepository>('SpaceRepository', ['getAll']);

    TestBed.configureTestingModule({
      providers: [
        GetSpacesUseCase,
        { provide: SPACE_REPOSITORY_TOKEN, useValue: mockRepository }
      ]
    });

    useCase = TestBed.inject(GetSpacesUseCase);
  });

  it('should successfully retrieve mapped space models from repository', (done) => {
    mockRepository.getAll.and.returnValue(of(mockSpaces));

    useCase.execute().subscribe({
      next: (result) => {
        expect(result.length).toBe(1);
        expect(result[0].id).toBe('sp-101');
        expect(result[0].title).toBe('Miraflores Office Hub');
        expect(mockRepository.getAll).toHaveBeenCalledTimes(1);
        done();
      },
      error: done.fail
    });
  });

  it('should handle repository exceptions and emit error', (done) => {
    mockRepository.getAll.and.returnValue(throwError(() => new Error('Service Unavailable')));

    useCase.execute().subscribe({
      next: () => done.fail('Expected error, but got success response'),
      error: (error) => {
        expect(error.message).toBe('Service Unavailable');
        done();
      }
    });
  });
});
