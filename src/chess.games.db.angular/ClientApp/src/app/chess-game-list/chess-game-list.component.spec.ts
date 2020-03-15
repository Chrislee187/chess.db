/// <reference path="../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { ChessGameListComponent } from './chess-game-list.component';

let component: ChessGameListComponent;
let fixture: ComponentFixture<ChessGameListComponent>;

describe('chess-game-list component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ ChessGameListComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(ChessGameListComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});
