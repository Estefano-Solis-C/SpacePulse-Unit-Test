import { SpaceAssembler } from '../../../../src/app/features/spaces/assemblers/space.assembler';
import { SpaceDto } from '../../../../src/app/features/spaces/models/space.dto';

describe('SpaceAssembler', () => {
  const sampleDto: SpaceDto = {
    id: 'sp-200',
    title: 'San Isidro Executive Loft',
    description: 'Smart executive loft with IoT climate control',
    type: 'Apartment',
    status: 'Published',
    pricePerMonth: 1800,
    totalPricing: 1800,
    location: {
      address: 'Calle Las Begonias 123',
      city: 'Lima',
      country: 'Peru',
      latitude: -12.095,
      longitude: -77.032
    },
    images: ['https://example.com/loft.png'],
    ownerId: 'usr-002'
  };

  it('should accurately transform SpaceDto into SpaceModel domain entity', () => {
    const model = SpaceAssembler.toModel(sampleDto);

    expect(model.id).toBe('sp-200');
    expect(model.title).toBe('San Isidro Executive Loft');
    expect(model.type).toBe('Apartment');
    expect(model.status).toBe('Published');
    expect(model.location.city).toBe('Lima');
    expect(model.location.address).toBe('Calle Las Begonias 123');
    expect(model.images.length).toBe(1);
    expect(model.ownerId).toBe('usr-002');
  });

  it('should fallback to default images when DTO images are empty or undefined', () => {
    const dtoWithoutImages = { ...sampleDto, images: [] };
    const model = SpaceAssembler.toModel(dtoWithoutImages);

    expect(model.images.length).toBeGreaterThan(0);
  });

  it('should gracefully handle empty or null collections in toModelList', () => {
    expect(SpaceAssembler.toModelList([])).toEqual([]);
    expect(SpaceAssembler.toModelList(null as any)).toEqual([]);
  });
});
