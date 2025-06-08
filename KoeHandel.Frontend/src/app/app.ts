import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Button } from "./button/button";
import { Home } from "./home/home";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Home],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'KoeHandel.Frontend';
}
