import './voteResultsDisplay.css'
import React from 'react'
import moment from 'moment'
import { Vote } from './types'
import { useDebounce } from 'use-debounce'

export type VoteResultsDisplayProps = {
	staticName: string
	dates: moment.Moment[]
	voted: boolean
	goVote: () => void
}

type VoteResult = {
	date: string
	votes: {
		gw2Acc: string
		disAcc: string
		vote: Vote
	}[]
}

function Voter(x: { gw2Acc: string, disAcc: string, className: string }) {
	const clNm = React.useMemo(() => `voter ${x.className}`, [x.className])
	return (
		<div className={clNm}>
			<span>{x.disAcc}</span>/<span>{x.gw2Acc}</span>
		</div>
	)
}

function VoteResultForDate({date, votes} : VoteResult) {
	const may = React.useMemo(() => votes.filter(x => x.vote == Vote.MayAttend), votes)
	const want = React.useMemo(() => votes.filter(x => x.vote == Vote.WantAttend), votes)

	return (
		<div className="vote-result">
			<h2>{date}</h2>
			<div>Желающих: {want.length + may.length}</div>
			<div className="want">Точно пойдут: {want.length}</div>
			<div className="may">Наверное пойдут: {may.length}</div>
			<div className="voters">
				{want.map(x => <Voter key={x.disAcc} className="want" {...x} />)}
				{may.map(x => <Voter key={x.disAcc} className="may" {...x} />)}
			</div>
		</div>
	)
}

export function VoteResultsDisplay({ staticName, dates, voted, goVote }: VoteResultsDisplayProps) {
	const [loading, setLoading] = React.useState(true)
	const setLoaded = React.useCallback(() => setLoading(false), [setLoading])
	const [results, setResults] = React.useState<VoteResult[]>([])

	React.useEffect(() => {
		const aborter = new AbortController()
		fetch('/api/results', {
			method: 'POST',
			signal: aborter.signal,
			body: JSON.stringify({ staticName, dates }),
			headers: {
				['content-type']: 'application/json'
			}
		}).then(x => x.json()).then((x: any[]) => x.map(x => ({ ...x, date: moment.utc(x.date) }))).then(setResults).finally(setLoaded)
		return () => aborter.abort()
	}, [42])

	const [filterDate, setFilterDate] = React.useState('')
	const [debouncedFilterDate] = useDebounce(filterDate, 300)
	const usefulResults = React.useMemo(() =>
		results
			.map(x => ({
				date: moment.utc(x.date).local().format("DD.MM.YYYY HHч"),
				votes: x.votes.filter(x => x.vote != Vote.CantAttend)
			}))
			.filter(x => x.votes.length > 0), [results])
	const filteredResults = React.useMemo(() => usefulResults.filter(x => x.date.startsWith(debouncedFilterDate)), [usefulResults, debouncedFilterDate])

	if (loading)
		return <span>Загружается...</span>

	if (usefulResults.length == 0)
		return (
			<>
				<div>Никто пока не проголосовал</div>
				<button type="button" onClick={goVote}>Проголосовать!</button>
			</>
		)

	return (
		<div className="vote-results">
			<div className="vote-results__filter">
				<input type="text" placeholder="Фильтр по дате" value={filterDate} onChange={e => setFilterDate(e.target.value)} />
			</div>
			<div className="vote-results__dates">
				{filteredResults.map(x => <VoteResultForDate key={x.date} {...x} />)}
			</div>
			<button type="button" onClick={goVote}>{voted ? 'Проголосовать заново' : 'Проголосовать'}</button>
		</div>
	)
}