import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList, IonFab, IonFabButton, IonModal, IonInput, IonSelect, IonSelectOption, IonDatetime, IonDatetimeButton, IonTextarea } from '@ionic/angular/standalone';
import { FormsModule } from '@angular/forms';
import { ClinicalApiService } from '../../core/services/clinical-api.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-citas',
  templateUrl: './citas.page.html',
  styleUrls: ['./citas.page.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList, IonFab, IonFabButton, IonModal, IonInput, IonSelect, IonSelectOption, IonDatetime, IonDatetimeButton, IonTextarea]
})
export class CitasPage implements OnInit {
  isLoading = true;
  citas: any[] = [];
  pokemons: any[] = [];
  entrenadores: any[] = [];
  isModalOpen = false;
  isCreating = false;

  nuevaCita = {
    fechaProgramadaUtc: new Date().toISOString(),
    motivo: '',
    entrenadorId: null,
    pokemonId: null
  };

  constructor(private api: ClinicalApiService, private auth: AuthService, private router: Router) {}

  ngOnInit() {
    this.loadCitas();
    if (this.canCreate()) {
      this.loadCatalogos();
    }
  }

  canCreate(): boolean {
    return this.auth.hasRole('Administrador', 'Recepcionista');
  }

  loadCatalogos() {
    this.api.getPokemons().subscribe({
      next: (data) => this.pokemons = data,
      error: () => this.pokemons = []
    });
    this.api.getEntrenadores().subscribe({
      next: (data) => this.entrenadores = data,
      error: () => this.entrenadores = []
    });
  }

  loadCitas() {
    this.isLoading = true;
    this.api.getCitas().subscribe({
      next: (data) => {
        this.citas = data;
        this.isLoading = false;
      },
      error: () => {
        this.citas = [];
        this.isLoading = false;
      }
    });
  }

  getEstadoColor(estado: string): string {
    if (estado === 'Confirmada') return 'success';
    if (estado === 'Pendiente') return 'warning';
    if (estado === 'Cancelada') return 'danger';
    return 'primary';
  }

  setOpen(isOpen: boolean) {
    this.isModalOpen = isOpen;
    if (!isOpen) {
      this.resetForm();
    }
  }

  resetForm() {
    this.nuevaCita = {
      fechaProgramadaUtc: new Date().toISOString(),
      motivo: '',
      entrenadorId: null,
      pokemonId: null
    };
  }

  crearCita() {
    if (!this.nuevaCita.motivo || !this.nuevaCita.entrenadorId || !this.nuevaCita.pokemonId) return;
    
    this.isCreating = true;
    this.api.createCita(this.nuevaCita).subscribe({
      next: () => {
        this.isCreating = false;
        this.setOpen(false);
        this.loadCitas();
      },
      error: () => {
        this.isCreating = false;
        alert('Error al crear la cita');
      }
    });
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}
