import { Component, input, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './input.html',
  styleUrl: './input.scss'
})
export class Input {
  label = input<string>('');
  placeholder = input<string>('');
  value = input<any>();
  @Output() valueChange = new EventEmitter<any>();
}
