import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { IonContent, IonInput, IonButton, IonSpinner, IonIcon } from '@ionic/angular/standalone';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, IonContent, IonInput, IonButton, IonSpinner, IonIcon]
})
export class LoginPage {
  correo = '';
  password = '';
  isLoading = false;
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    this.errorMessage = '';
    this.isLoading = true;

    this.authService.login(this.correo, this.password).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/pokemon-list']);
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error?.error?.mensaje || error?.message || 'Error al iniciar sesion';
      }
    });
  }
}
