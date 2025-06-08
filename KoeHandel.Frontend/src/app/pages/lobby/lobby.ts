import { Component } from '@angular/core';
import { Title } from "../../components/title/title";
import { Loading } from "../../components/loading/loading";

@Component({
  selector: 'app-lobby',
  imports: [Title, Loading],
  templateUrl: './lobby.html',
  styleUrl: './lobby.scss'
})
export class Lobby {

}
