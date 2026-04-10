import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardHeader, IonCardTitle, IonCardSubtitle, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner } from '@ionic/angular/standalone';
import { AuthService } from '../../core/services/auth.service';
import { ClinicalApiService } from '../../core/services/clinical-api.service';

@Component({
  selector: 'app-pokemon-list',
  templateUrl: './pokemon-list.page.html',
  styleUrls: ['./pokemon-list.page.scss'],
  standalone: true,
  imports: [CommonModule, RouterLink, IonHeader, IonToolbar, IonTitle, IonContent, IonCard, IonCardHeader, IonCardTitle, IonCardSubtitle, IonCardContent, IonBadge, IonButton, IonIcon, IonButtons, IonSpinner]
})
export class PokemonListPage implements OnInit {
  trainer: any;
  pokemons: any[] = [];
  isLoading = true;

  constructor(private api: ClinicalApiService, private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.trainer = this.authService.getUser();
    this.loadPokemons();
  }

  loadPokemons() {
    this.isLoading = true;
    this.api.getPokemons().subscribe({
      next: (data) => {
        this.pokemons = data;
        this.isLoading = false;
      },
      error: () => {
        this.pokemons = [];
        this.isLoading = false;
      }
    });
  }

  canView(module: string): boolean {
    const role = this.authService.getRole();
    const access: Record<string, string[]> = {
      tratamientos: ['Administrador', 'Enfermero', 'Entrenador'],
      citas: ['Administrador', 'Enfermero', 'Entrenador'],
      'estado-clinico': ['Administrador', 'Enfermero', 'Entrenador'],
      comprobantes: ['Administrador', 'Enfermero', 'Entrenador']
    };

    return (access[module] || []).includes(role);
  }

  goToDetail(pokemon: any) {
    this.router.navigate(['/pokemon-detail', pokemon.id]);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
