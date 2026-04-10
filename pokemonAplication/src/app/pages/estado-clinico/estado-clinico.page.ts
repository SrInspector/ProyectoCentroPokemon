import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList } from '@ionic/angular/standalone';
import { forkJoin } from 'rxjs';
import { ClinicalApiService } from '../../core/services/clinical-api.service';

@Component({
  selector: 'app-estado-clinico',
  templateUrl: './estado-clinico.page.html',
  styleUrls: ['./estado-clinico.page.scss'],
  standalone: true,
  imports: [CommonModule, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner, IonItem, IonLabel, IonList]
})
export class EstadoClinicoPage implements OnInit {
  isLoading = true;
  pokemons: any[] = [];

  constructor(private api: ClinicalApiService, private router: Router) {}

  ngOnInit() {
    this.loadPokemons();
  }

  loadPokemons() {
    this.isLoading = true;
    forkJoin({
      pokemons: this.api.getPokemons(),
      tratamientos: this.api.getTratamientos(),
      citas: this.api.getCitas()
    }).subscribe({
      next: ({ pokemons, tratamientos, citas }) => {
        this.pokemons = pokemons.map((pokemon) => ({
          ...pokemon,
          tratamientosActivos: tratamientos.filter((item) => item.pokemonId === pokemon.id && item.estado === 'Activo').length,
          proximaCita: citas.find((item) => item.pokemonId === pokemon.id)?.fechaProgramadaUtc || null
        }));
        this.isLoading = false;
      },
      error: () => {
        this.pokemons = [];
        this.isLoading = false;
      }
    });
  }

  getEstadoColor(estado: string): string {
    if (estado === 'Estable') return 'success';
    if (estado === 'Hospitalizado' || estado === 'EnTratamiento') return 'warning';
    if (estado === 'Critico') return 'danger';
    return 'medium';
  }

  goBack() {
    this.router.navigate(['/pokemon-list']);
  }
}
