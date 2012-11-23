using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AudioLib.PortAudioInterop
{
    public enum StreamState
    {
        Closed,
        Open,
        Stopped,
        Started
    }

    public class RealtimeHost : IDisposable
    {
        private static RealtimeHost _host;
        public static RealtimeHost Host
        {
            get
            {
                if (_host == null)
                    _host = new RealtimeHost();

                return _host;
            }
        }

        /// <summary>
        /// StreamState is used to determine the current state of the host. Before making changes to the configuration the state must be Closed.
        /// </summary>
        public StreamState StreamState { get; internal set; }

        /// <summary>
        /// Configuration info for the PortAudio Stream. Note that it is not allowed to change parameters in the config unless the StreamState is Closed
        /// </summary>
        RealtimeHostConfig Config;

        private RealtimeHost()
        {
            PortAudio.Pa_Initialize();
            StreamState = StreamState.Closed;
        }

        public void Dispose()
        {
            PortAudio.Pa_Terminate();
        }

        public void SetConfig(RealtimeHostConfig config)
        {
            if (config == null)
                throw new ArgumentNullException();

            var originalState = StreamState;

            if (StreamState == StreamState.Started)
                StopStream();
            if (StreamState == StreamState.Stopped)
                CloseStream();

            if (StreamState != StreamState.Closed)
                throw new Exception("Stream could not be closed, unable to change settings!");

            Config = config;

            if (originalState == StreamState.Started || originalState == StreamState.Open)
                OpenStream();

            if (originalState == StreamState.Started)
                StartStream();
        }

        // ------ Properties from PortAudioConfig

        public int Samplerate { get { return Config.Samplerate; } }
        public int NumberOfInputs { get { return Config.NumberOfInputs; } }
        public int NumberOfOutputs { get { return Config.NumberOfOutputs; } }
        public string InputDeviceName { get { return Config.InputDeviceName; } }
        public string OutputDeviceName { get { return Config.OutputDeviceName; } }
        public UInt32 BufferSize { get { return Config.BufferSize; } }
        public string APIName { get { return Config.APIName; } }
        public double InputLatency { get { return Config.InputLatencyMs; } }
        public double OutputLatency { get { return Config.OutputLatencyMs; } }

        // --------------------------------------

        public Action<float[][], float[][]> Process;

        PortAudio.PaStreamCallbackResult RealtimeCallback(IntPtr inputBuffer, IntPtr outputBuffer, uint framesPerBuffer, ref PortAudio.PaStreamCallbackTimeInfo timeInfo, PortAudio.PaStreamCallbackFlags statusFlags, IntPtr userData)
        {
            unsafe
            {
                float[][] Inputs = new float[NumberOfInputs][];
                float[][] Outputs = new float[NumberOfOutputs][];

                // create input channels and copy data into them
                for (int i = 0; i < NumberOfInputs; i++)
                {
                    float* inChannel = (float*)((float**)inputBuffer)[i];
                    float[] inArr = new float[framesPerBuffer];
                    Marshal.Copy((IntPtr)inChannel, inArr, 0, (int)framesPerBuffer);

                    Inputs[i] = inArr;
                }

                // create empty output array
                for (int i = 0; i < NumberOfOutputs; i++)
                {
                    float[] outArr = new float[framesPerBuffer];
                    Outputs[i] = outArr;
                }

                // Call the process function
                Process(Inputs, Outputs);

                // Copy data back from managed array into native array
                for (int i = 0; i < NumberOfOutputs; i++)
                {
                    float* outChannel = (float*)((float**)outputBuffer)[i];
                    float[] outArr = Outputs[i];
                    Marshal.Copy(outArr, 0, (IntPtr)outChannel, (int)framesPerBuffer);
                }

                return PortAudio.PaStreamCallbackResult.paContinue;
            }
        }

        void StreamFinished(IntPtr userData)
        {
            Console.WriteLine("Exit");
        }

        // ----------------------------- PortAudio Operations -----------------------------

        PortAudio.PaStreamCallbackDelegate callbackDelegate;
        PortAudio.PaStreamFinishedCallbackDelegate streamFinishedDelegate;

        public void OpenStream()
        {
            if (Config == null)
                throw new Exception("Configuration settings have not been applied. You must first configure the host");

            if (StreamState != StreamState.Closed)
                throw new Exception("Trying to Open a stream that does not have State: Closed. State is " + StreamState.ToString());

            callbackDelegate = RealtimeCallback;
            streamFinishedDelegate = StreamFinished;

            var err = PortAudio.Pa_OpenStream(
                        out Config.Stream,
                        ref Config.inputParameters,
                        ref Config.outputParameters,
                        Samplerate,
                        BufferSize,
                        (PortAudio.PaStreamFlags.paClipOff | PortAudio.PaStreamFlags.paDitherOff),
                        callbackDelegate,
                        new IntPtr(0)
                );

            if (err != PortAudio.PaError.paNoError)
                throw new Exception(PortAudio.Pa_GetErrorText(err));

            err = PortAudio.Pa_SetStreamFinishedCallback(Config.Stream, streamFinishedDelegate);

            if (err != PortAudio.PaError.paNoError)
                throw new Exception(PortAudio.Pa_GetErrorText(err));

            StreamState = StreamState.Open;
        }

        public void StartStream()
        {
            if (StreamState != StreamState.Open)
                throw new Exception("Trying to Start a stream that does not have State: Open. State is " + StreamState.ToString());

            var err = PortAudio.Pa_StartStream(Config.Stream);
            if (err != PortAudio.PaError.paNoError)
                throw new Exception(PortAudio.Pa_GetErrorText(err));

            StreamState = StreamState.Started;
        }

        public void StopStream()
        {
            if (StreamState != StreamState.Started)
                throw new Exception("Trying to Stop a stream that does not have State: Started. State is " + StreamState.ToString());

            if (Config == null)
                return;

            var err = PortAudio.Pa_StopStream(Config.Stream);
            if (err != PortAudio.PaError.paNoError)
                throw new Exception(PortAudio.Pa_GetErrorText(err));

            StreamState = StreamState.Stopped;
        }

        public void CloseStream()
        {
            if (StreamState != StreamState.Stopped)
                throw new Exception("Trying to Close a stream that does not have State: Stopped. State is " + StreamState.ToString());

            if (Config == null)
                return;

            var err = PortAudio.Pa_CloseStream(Config.Stream);
            if (err != PortAudio.PaError.paNoError)
                throw new Exception(PortAudio.Pa_GetErrorText(err));

            StreamState = StreamState.Closed;
        }
    }
}
