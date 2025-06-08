import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { tap, timer } from 'rxjs';

@Component({
  selector: 'app-loading',
  imports: [AsyncPipe],
  templateUrl: './loading.html',
  styleUrl: './loading.scss'
})
export class Loading {
  seconds$ = timer(500, 500)
  .pipe(tap(console.log));
}
