import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList, IonFab, IonFabButton, IonModal, IonInput, IonSelect, IonSelectOption, IonDatetime, IonDatetimeButton, IonToggle } from '@ionic/angular/standalone';
import { FormsModule } from '@angular/forms';
import { ClinicalApiService } from '../../core/services/clinical-api.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-tratamientos',
  templateUrl: './tratamientos.page.html',
  styleUrls: ['./tratamientos.page.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList, IonFab, IonFabButton, IonModal, IonInput, IonSelect, IonSelectOption, IonDatetime, IonDatetimeButton, IonToggle]
})
export class TratamientosPage implements OnInit {
  isLoading = true;
  tratamientos: any[] = [];
  pokemons: any[] = [];
  isModalOpen = false;
  isCreating = false;

  nuevoTratamiento = {
    pokemonId: null,
    tipo: '',
    dosis: '',
    frecuencia: '',
    fechaInicioUtc: new Date().toISOString(),
    fechaFinUtc: new Date().toISOString(),
    esCritico: false
  };

  constructor(private api: ClinicalApiService, private auth: AuthService, private router: Router) {}

  ngOnInit() {
    this.loadTratamientos();
    if (this.canCreate()) {
      this.loadCatalogos();
    }
  }

  canCreate(): boolean {
    return this.auth.hasRole('Administrador', 'Enfermero');
  }

  loadCatalogos() {
    this.api.getPokemons().subscribe({
      next: (data) => this.pokemons = data,
      error: () => this.pokemons = []
    });
  }

  loadTratamientos() {
    this.isLoading = true;
    this.api.getTratamientos().subscribe({
      next: (data) => {
        this.tratamientos = data;
        this.isLoading = false;
      },
      error: () => {
        this.tratamientos = [];
        this.isLoading = false;
      }
    });
  }

  getEstadoColor(estado: string): string {
    if (estado === 'Activo') return 'warning';
    if (estado === 'Finalizado') return 'success';
    if (estado === 'Cancelado') return 'danger';
    return 'medium';
  }

  setOpen(isOpen: boolean) {
    this.isModalOpen = isOpen;
    if (!isOpen) {
      this.resetForm();
    }
  }

  resetForm() {
    this.nuevoTratamiento = {
      pokemonId: null,
      tipo: '',
      dosis: '',
      frecuencia: '',
      fechaInicioUtc: new Date().toISOString(),
      fechaFinUtc: new Date().toISOString(),
      esCritico: false
    };
  }

  crearTratamiento() {
    if (!this.nuevoTratamiento.pokemonId || !this.nuevoTratamiento.tipo || !this.nuevoTratamiento.dosis || !this.nuevoTratamiento.frecuencia) return;
    
    this.isCreating = true;
    this.api.createTratamiento(this.nuevoTratamiento).subscribe({
      next: () => {
        this.isCreating = false;
        this.setOpen(false);
        this.loadTratamientos();
      },
      error: () => {
        this.isCreating = false;
        alert('Error al crear el tratamiento');
      }
    });
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}
