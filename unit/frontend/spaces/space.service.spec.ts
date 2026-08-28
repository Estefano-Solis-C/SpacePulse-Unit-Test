import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SpaceService } from '../../../../src/app/features/spaces/services/space.service';
import { environment } from '../../../../src/environments/environment';
import { CreateSpaceDto, SpaceDto } from '../../../../src/app/features/spaces/models/space.dto';

describe('SpaceService (HTTP Repository Implementation)', () => {
  let service: SpaceService;
  let httpMock: HttpTestingController;

  const sampleDtos: SpaceDto[] = [
    {
      id: 'sp-1',
      title: 'Smart Hub 1',
      description: 'Smart Space',
      type: 'Office',
      status: 'Published',
      pricePerMonth: 1200,
      totalPricing: 1200,
      location: {
        address: 'Av. Arequipa 100',
        city: 'Lima',
        country: 'Peru',
        latitude: -12.08,
        longitude: -77.03
      },
      images: [],
      ownerId: 'usr-1'
    }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SpaceService]
    });

    service = TestBed.inject(SpaceService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch all spaces from backend via GET /spaces', () => {
    service.getAll().subscribe((models) => {
      expect(models.length).toBe(1);
      expect(models[0].id).toBe('sp-1');
      expect(models[0].title).toBe('Smart Hub 1');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/space`);
    expect(req.request.method).toBe('GET');
    req.flush(sampleDtos);
  });

  it('should create new space via POST /spaces', () => {
    const newDto: CreateSpaceDto = {
      title: 'New Co-working',
      description: 'Desc',
      type: 'Office',
      pricePerMonth: 1500,
      location: sampleDtos[0].location,
      images: []
    };

    service.create(newDto).subscribe((model) => {
      expect(model.id).toBe('sp-new');
      expect(model.title).toBe('New Co-working');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/space`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newDto);
    req.flush({ ...sampleDtos[0], id: 'sp-new', title: 'New Co-working' });
  });

  it('should send accept request via POST /spaces/{id}/accept', () => {
    service.accept('sp-1').subscribe((result) => {
      expect(result.id).toBe('sp-1');
      expect(result.status).toBe('Accepted');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/space/sp-1/accept`);
    expect(req.request.method).toBe('POST');
    req.flush({ message: 'Success', data: { ...sampleDtos[0], status: 'Accepted' } });
  });
});
