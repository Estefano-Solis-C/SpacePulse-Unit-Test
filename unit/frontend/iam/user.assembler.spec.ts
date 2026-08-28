import { UserAssembler } from '../../../../src/app/features/iam/assemblers/user.assembler';
import { UserDto, LoginResponseDto } from '../../../../src/app/features/iam/models/user.dto';

describe('UserAssembler', () => {
  const userDto: UserDto = {
    id: 'usr-100',
    fullName: 'Maria Rodriguez',
    email: 'maria@spacepulse.com',
    phone: '+51 987654321',
    role: 'Homeowner',
    paymentMethods: [
      {
        id: 1,
        userId: 'usr-100',
        type: 'Visa',
        number: '4111222233334444',
        expiry: '12/28',
        cvv: '123'
      }
    ]
  };

  it('should transform UserDto into UserModel with masked credit cards', () => {
    const model = UserAssembler.toModel(userDto);
    expect(model.id).toBe('usr-100');
    expect(model.fullName).toBe('Maria Rodriguez');
    expect(model.role).toBe('Homeowner');
    expect(model.paymentMethods.length).toBe(1);
    expect(model.paymentMethods[0].maskedNumber).toBe('•••• •••• •••• 4444');
  });

  it('should transform LoginResponseDto to AuthSessionModel', () => {
    const loginRes: LoginResponseDto = {
      token: 'jwt-mock-token-xyz',
      id: 'usr-200',
      fullName: 'David Builder',
      email: 'david@spacepulse.com',
      role: 'Remodeler'
    };

    const session = UserAssembler.fromLoginResponse(loginRes);
    expect(session.token).toBe('jwt-mock-token-xyz');
    expect(session.user.role).toBe('Remodeler');
    expect(session.user.fullName).toBe('David Builder');
  });
});
