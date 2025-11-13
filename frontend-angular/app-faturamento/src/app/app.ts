import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet } from '@angular/router';

// --- Minhas importações do Angular Material ---
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
// --- Fim das importações ---

@Component({
  selector: 'app-root',
  standalone: true,
  
  // Adiciono os módulos do Material e o RouterLink em 'imports'
  imports: [
    CommonModule, 
    RouterOutlet,
    RouterLink, // Permite usar [routerLink]
    MatToolbarModule, // Fornece <mat-toolbar>
    MatButtonModule // Fornece botões estilizados do Material
  ],

  templateUrl: './app.html', // apontando para o arquivo HTML
  styleUrls: ['./app.scss']     // apontando para o arquivo SCSS
})
export class App { // O nome da classe é 'App', como o 'main.ts' espera
  title = 'app-faturamento';
}