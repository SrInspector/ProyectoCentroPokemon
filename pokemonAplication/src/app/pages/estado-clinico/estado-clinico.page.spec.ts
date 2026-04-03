import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EstadoClinicoPage } from './estado-clinico.page';

describe('EstadoClinicoPage', () => {
  let component: EstadoClinicoPage;
  let fixture: ComponentFixture<EstadoClinicoPage>;

  beforeEach(() => {
    fixture = TestBed.createComponent(EstadoClinicoPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
