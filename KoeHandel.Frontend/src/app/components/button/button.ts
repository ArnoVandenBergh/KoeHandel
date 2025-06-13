import { Component, input, Input, output, Output } from '@angular/core';

@Component({
  selector: 'app-button',
  imports: [],
  templateUrl: './button.html',
  styleUrl: './button.scss'
})
export class Button {
  buttonText = input<string>();
  onClick = output<void>();
  disabled = input<boolean>(false);
}
