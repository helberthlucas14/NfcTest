import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { ReactiveFormsModule, FormBuilder, Validators, FormArray, FormGroup } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { NotaFiscal } from '../core/models';

@Component({
  selector: 'app-edit-nota-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
  ],
  templateUrl: './edit-nota-dialog.component.html',
})
export class EditNotaDialogComponent {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private ref: MatDialogRef<EditNotaDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data?: { nota?: NotaFiscal }
  ) {

    this.form = this.fb.group({
      emissor: ['', Validators.required],
      data: [new Date(), Validators.required],
      items: this.fb.array([] as FormGroup[]),
    });

    if (data?.nota) {
      const d = data.nota;
      const dt = d.dataEmissao ? new Date(d.dataEmissao) : new Date();
      this.form.patchValue({ emissor: d.emissor, data: dt });
      const arr = this.itemsArray();
      (d.itens || []).forEach(it => {
        arr.push(this.fb.group({
          id: [it.id],
          descricao: [it.descricao, Validators.required],
          valor: [it.valor, [Validators.required, Validators.min(0)]]
        }));
      });
    }
  }

  itemsArray(): FormArray {
    return this.form.get('items') as FormArray;
  }

  itemsControls(): FormGroup[] {
    return this.itemsArray().controls as FormGroup[];
  }

  addItem() {
    this.itemsArray().push(this.fb.group({
      descricao: ['', Validators.required],
      valor: [0, [Validators.required, Validators.min(0)]]
    }));
  }

  removeItem(index: number) {
    this.itemsArray().removeAt(index);
  }

  salvar() {
    const value = this.form.value;
    const d = value.data as Date;
    const pad = (n: number) => n.toString().padStart(2, '0');
    const dateOnly = `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
    const items = (value.items as any[] | undefined)?.map(i => ({ id: i.id, descricao: i.descricao, valor: Number(i.valor) })) || [];
    this.ref.close({ emissor: value.emissor!, dataEmissao: dateOnly, items });
  }

  cancelar() {
    this.ref.close();
  }
}
