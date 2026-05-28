import { Component, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HistoriquesService } from '../../services/historiques.service';
import { timeout, catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';


type Phase = 'IDLE' | 'IN' | 'HOLD' | 'OUT';

interface BreathingPreset {
  code: string;
  label: string;
  inspire: number;
  hold: number;
  expire: number;
}

@Component({
  selector: 'app-breathing',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './breathing.html'
})
export class BreathingComponent implements OnDestroy {

  /* ---------------- presets CDC ---------------- */

  presets: BreathingPreset[] = [
    { code: '748', label: '7-4-8', inspire: 7, hold: 4, expire: 8 },
    { code: '55', label: '5-5', inspire: 5, hold: 0, expire: 5 },
    { code: '46', label: '4-6', inspire: 4, hold: 0, expire: 6 }
  ];

  selected = signal<BreathingPreset | null>(null);

  /* ---------------- player state ---------------- */

  phase = signal<Phase>('IDLE');
  remaining = signal<number>(0);
  running = signal(false);

  message = signal<string | undefined>(undefined);
  saving = signal(false);

  private timer: any = null;

  constructor(private historiquesService: HistoriquesService) {}

  ngOnDestroy(): void {
    this.clearTimer();
  }

  /* ---------------- UI helpers ---------------- */

  phaseLabel(): string {
    const p = this.phase();
    if (p === 'IN') return 'Inspire';
    if (p === 'HOLD') return 'Apnée';
    if (p === 'OUT') return 'Expire';
    return 'Prêt';
  }

  /* ---------------- preset selection ---------------- */

  selectPreset(code: string): void {
    const p = this.presets.find(x => x.code === code) ?? null;
    this.selected.set(p);
    this.reset();
  }

  /* ---------------- player controls ---------------- */

  toggle(): void {
    if (!this.selected()) {
      this.message.set("Sélectionnez un exercice.");
      return;
    }

    if (this.running()) {
      this.pause();
    } else {
      this.start();
    }
  }

  start(): void {
    const preset = this.selected();
    if (!preset) return;

    this.running.set(true);
    this.phase.set('IN');
    this.remaining.set(preset.inspire);

    this.timer = setInterval(() => this.tick(), 1000);
  }

  pause(): void {
    this.running.set(false);
    this.clearTimer();
  }

  reset(): void {
    this.pause();
    this.phase.set('IDLE');
    this.remaining.set(0);
  }

  private tick(): void {
    const preset = this.selected();
    if (!preset) return;

    const r = this.remaining();

    if (r > 1) {
      this.remaining.set(r - 1);
      return;
    }

    const phase = this.phase();

    if (phase === 'IN') {
      if (preset.hold > 0) {
        this.phase.set('HOLD');
        this.remaining.set(preset.hold);
      } else {
        this.phase.set('OUT');
        this.remaining.set(preset.expire);
      }
      return;
    }

    if (phase === 'HOLD') {
      this.phase.set('OUT');
      this.remaining.set(preset.expire);
      return;
    }

    if (phase === 'OUT') {
      this.phase.set('IN');
      this.remaining.set(preset.inspire);
      return;
    }
  }

  private clearTimer(): void {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }

  /* ---------------- save manually ---------------- */

saveManual(): void {

  const preset = this.selected();

  if (!preset) {
    this.message.set("Choisissez un exercice avant d'enregistrer.");
    return;
  }

  this.saving.set(true);
  this.message.set("Enregistrement en cours...");

  const duration = preset.inspire + preset.hold + preset.expire;

  this.historiquesService.createHistorique({
    exerciceId: 1,
    dureeEffectiveSec: duration
  }).pipe(
    timeout(5000),
    catchError(err => {

      if (err.status === 401) {
        this.message.set("Session expirée, reconnectez-vous.");
      } else if (err.status === 0) {
        this.message.set("Serveur indisponible.");
      } else {
        this.message.set("Impossible d'enregistrer la session.");
      }

      return of(null);
    }),
    finalize(() => this.saving.set(false))
  ).subscribe(res => {
    if (res) {
      this.message.set("Session enregistrée ✔");
    }
  });
}
}