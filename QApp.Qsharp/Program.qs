// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace QuantumRNG {

    open Microsoft.Quantum.Intrinsic;
    open Microsoft.Quantum.Measurement;
    open Microsoft.Quantum.Canon;

    @EntryPoint()
    operation GenerateRandomBits(n : Int) : Result[] {
        use qubits = Qubit[n];
        ApplyToEach(H, qubits);
        return MultiM(qubits);
    }
}
